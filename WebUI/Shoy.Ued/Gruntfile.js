module.exports = function (grunt) {
    // 配置
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        //js压缩
        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> version:<%= pkg.version %> <%= grunt.template.today("yyyy-mm-dd HH:MM:ss") %> */\n'
            },
            list: {
                files: [
                    {
                        expand: true,
                        cwd: 'source/js',
                        src: ['**/*.js'],
                        dest: 'js',
                        ext: '.min.js'
                    }
//                    ,{
//                        expand:true,
//                        cwd:'source/plugs',
//                        src:['**/*.js'],
//                        dest:'plugs'
//                    }
                ]
            }
        },
        //css压缩
        cssmin: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd HH:MM:ss") %> */'
            },
            combine: {
                files: [
                    {
                        expand: true,
                        cwd: 'source/css',
                        src: ['**/*.css'],
                        dest: 'css',
                        ext: '.min.css'
                    }
//                    ,{
//                        expand:true,
//                        cwd:'source/plugs',
//                        src:['**/*.css'],
//                        dest:'plugs'
//                    }
                ]
            }
        },
        //文件监视 执行命令 grunt watch
        watch: {
            js: {
                files: ['source/js/**/*.js'],
                tasks: ['uglify']
            },
            css: {
                files: ['source/css/**/*.css'],
                tasks: ['cssmin']
            }
        }
    });
    // 载入concat和uglify插件，分别对于合并和压缩
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-watch');
    // 注册任务
    grunt.registerTask('default', ['uglify', 'cssmin']);
}; 